using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Essentions.Components
{
    /// <summary> Contains a set of range methods. </summary>
    public static class Ranges
    {
        /// <summary> Coalesces consecutively contiguous ranges. You may want to sort ranges in
        /// <see cref="Range{T}.Min"/> to <see cref="Range{T}.Max"/> order. </summary>
        /// <typeparam name="T">The range key type.</typeparam>
        /// <param name="ranges">The ranges to coalesce.</param>
        /// <returns>The coalesced ranges.</returns>
        [NotNull]
        public static IEnumerable<Range<T>> Coalesce<T>([NotNull] this IEnumerable<Range<T>> ranges)
            where T : IComparable<T>
        {
            Check.NotNull(ranges);

            Range<T> previous = null;
            foreach (var range in ranges.Sort())
                if (previous == null) {
                    previous = range;
                } else {
                    if (previous.IsContiguousWith(range)) {
                        previous = previous.Union(range);
                    } else {
                        yield return previous;
                        previous = range;
                    }
                }

            if (previous != null) yield return previous;
        }

        /// <summary> Coalesces consecutively contiguous ranges. </summary>
        /// <typeparam name="TKey">The range key type.</typeparam>
        /// <typeparam name="TValue">The range value type.</typeparam>
        /// <param name="ranges">The ranges to coalesce.</param>
        /// <returns>The coalesced ranges.</returns>
        [NotNull]
        public static IEnumerable<Range<TKey>> Coalesce<TKey, TValue>(
            [NotNull] this IEnumerable<RangeWithValue<TKey, TValue>> ranges) where TKey : IComparable<TKey>
        {
            Check.NotNull(ranges);

            return ranges.MakeCovariant().Coalesce();
        }

        /// <summary> Indicates if the ranges contain the range. </summary>
        /// <typeparam name="T">The range type.</typeparam>
        /// <param name="ranges">The ranges to look in.</param>
        /// <param name="from">The start to look for.</param>
        /// <param name="to">The end to look for.</param>
        /// <returns>true if <code>ranges</code> contain the range, false otherwise.</returns>
        public static bool Contains<T>([NotNull] this IEnumerable<Range<T>> ranges, [NotNull] T from, [NotNull] T to)
            where T : IComparable<T>
        {
            Check.NotNull(ranges)
                 .LessOrEqual(from, to);

            return ranges.Contains(new Range<T>(from, to));
        }

        /// <summary> Indicates if the ranges contain the range. </summary>
        /// <typeparam name="TKey">The range key type.</typeparam>
        /// <typeparam name="TValue">THe range value type.</typeparam>
        /// <param name="ranges">The ranges to look in.</param>
        /// <param name="from">The start to look for.</param>
        /// <param name="to">The end to look for.</param>
        /// <returns>true if <code>ranges</code> contain the range, false otherwise.</returns>
        public static bool Contains<TKey, TValue>(
            this IEnumerable<RangeWithValue<TKey, TValue>> ranges,
            [NotNull] TKey from,
            [NotNull] TKey to) where TKey : IComparable<TKey>
        {
            Check.NotNull(ranges);

            return ranges.MakeCovariant().Contains(from, to);
        }

        /// <summary>
        ///     Indicates if the ranges contain the range.
        /// </summary>
        /// <typeparam name="T">The range type.</typeparam>
        /// <param name="ranges">The ranges to look in.</param>
        /// <param name="range">The item to look for.</param>
        /// <returns>true if <code>ranges</code> contain <code>range</code>, false otherwise.</returns>
        public static bool Contains<T>([NotNull] this IEnumerable<Range<T>> ranges, [NotNull] Range<T> range)
            where T : IComparable<T>
        {
            Check.NotNull(ranges)
                 .NotNull(range);

            return ranges.Overlapped(range).Coalesce().Any(item => item.Contains(range));
        }

        /// <summary>
        ///     Indicates if the ranges contain the range.
        /// </summary>
        /// <typeparam name="TKey">The range key type.</typeparam>
        /// <typeparam name="TValue">The range value type.</typeparam>
        /// <param name="ranges">The ranges to look in.</param>
        /// <param name="range">The item to look for.</param>
        /// <returns>true if <code>ranges</code> contain <code>range</code>, false otherwise.</returns>
        public static bool Contains<TKey, TValue>(
            [NotNull] this IEnumerable<RangeWithValue<TKey, TValue>> ranges,
            [NotNull] Range<TKey> range) where TKey : IComparable<TKey>
        {
            Check.NotNull(ranges)
                 .NotNull(range);

            return ranges.MakeCovariant().Contains(range);
        }

        /// <summary>
        ///     Indicates if the ranges contain the item.
        /// </summary>
        /// <typeparam name="T">The range type.</typeparam>
        /// <param name="ranges">The ranges to look in.</param>
        /// <param name="item">The item to look for.</param>
        /// <returns>true if <code>ranges</code> contain <code>item</code>, false otherwise.</returns>
        public static bool Contains<T>([NotNull] this IEnumerable<Range<T>> ranges, [NotNull] T item)
            where T : IComparable<T>
        {
            Check.NotNull(ranges);

            return ranges.Any(range => range.Contains(item));
        }

        /// <summary>
        ///     Indicates if the ranges contain the item.
        /// </summary>
        /// <typeparam name="TKey">The range key type.</typeparam>
        /// <typeparam name="TValue">The range value type.</typeparam>
        /// <param name="ranges">The ranges to look in.</param>
        /// <param name="item">The item to look for.</param>
        /// <returns>true if <code>ranges</code> contain <code>item</code>, false otherwise.</returns>
        public static bool Contains<TKey, TValue>(
            [NotNull] this IEnumerable<RangeWithValue<TKey, TValue>> ranges,
            [NotNull] TKey item) where TKey : IComparable<TKey>
        {
            Check.NotNull(ranges);

            return ranges.MakeCovariant().Contains(item);
        }

        /// <summary> Searches a set of ranges, and returns the first matching items. </summary>
        /// <typeparam name="TKey">The type of range.</typeparam>
        /// <typeparam name="TValue">The range value type.</typeparam>
        /// <param name="ranges">The ranges to search.</param>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The first matching range, or null.</returns>
        [CanBeNull]
        public static RangeWithValue<TKey, TValue> FindFirst<TKey, TValue>(
            [NotNull] this IEnumerable<RangeWithValue<TKey, TValue>> ranges,
            [NotNull] Func<Range<TKey>, bool> predicate) where TKey : IComparable<TKey>
        {
            Check.NotNull(ranges)
                 .NotNull(predicate);

            return ranges.MakeCovariant().Where(predicate).FirstOrDefault() as RangeWithValue<TKey, TValue>;
        }

        /// <summary> Fetches the ranges which are overlapped by this range. </summary>
        /// <typeparam name="T">The type of range.</typeparam>
        /// <param name="ranges">The ranges.</param>
        /// <param name="range">The range to test for overlappping.</param>
        /// <returns>The overlapped ranges.</returns>
        [NotNull]
        public static IEnumerable<Range<T>> Overlapped<T>(
            [NotNull] this IEnumerable<Range<T>> ranges,
            [NotNull] Range<T> range) where T : IComparable<T>
        {
            Check.NotNull(ranges)
                 .NotNull(range);

            return ranges.Where(item => item.Overlaps(range));
        }

        /// <summary> Fetches the ranges which are overlapped by this range. </summary>
        /// <typeparam name="TKey">The type of range.</typeparam>
        /// <typeparam name="TValue">The range value type.</typeparam>
        /// <param name="ranges">The ranges.</param>
        /// <param name="range">The range to test for overlappping.</param>
        /// <returns>The overlapped ranges.</returns>
        [NotNull]
        public static IEnumerable<Range<TKey>> Overlapped<TKey, TValue>(
            [NotNull] this IEnumerable<RangeWithValue<TKey, TValue>> ranges,
            [NotNull] Range<TKey> range) where TKey : IComparable<TKey>
        {
            Check.NotNull(ranges)
                 .NotNull(range);

            return ranges.MakeCovariant().Overlapped(range);
        }

        /// <summary>
        ///     Sorts the ranges.
        /// </summary>
        /// <typeparam name="T">The type of range.</typeparam>
        /// <param name="ranges">The sorted ranges.</param>
        /// <returns>The sorted ranges.</returns>
        [NotNull]
        public static IEnumerable<Range<T>> Sort<T>([NotNull] this IEnumerable<Range<T>> ranges)
            where T : IComparable<T>
        {
            Check.NotNull(ranges);

            var list = new List<Range<T>>(ranges);
            list.Sort();
            return list;
        }

        /// <summary>
        ///     Sorts the ranges.
        /// </summary>
        /// <typeparam name="TKey">The range key type.</typeparam>
        /// <typeparam name="TValue">The range value type.</typeparam>
        /// <param name="ranges">The sorted ranges.</param>
        /// <returns>The sorted ranges.</returns>
        [NotNull]
        public static IEnumerable<Range<TKey>> Sort<TKey, TValue>(
            [NotNull] this IEnumerable<RangeWithValue<TKey, TValue>> ranges) where TKey : IComparable<TKey>
        {
            Check.NotNull(ranges);

            return ranges.MakeCovariant().Sort();
        }

        /// <summary> Searches a set of ranges, and returns the matching items. </summary>
        /// <typeparam name="TKey">The type of range.</typeparam>
        /// <typeparam name="TValue">The range value type.</typeparam>
        /// <param name="ranges">The ranges to search.</param>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The matching ranges.</returns>
        [NotNull]
        public static IEnumerable<Range<TKey>> Where<TKey, TValue>(
            [NotNull] this IEnumerable<RangeWithValue<TKey, TValue>> ranges,
            [NotNull] Func<Range<TKey>, bool> predicate) where TKey : IComparable<TKey>
        {
            Check.NotNull(ranges)
                 .NotNull(predicate);

            return ranges.MakeCovariant().Where(predicate);
        }

        /// <summary> Makes a Range{TKey, TValue} enumerator covariant with Range{TKey} </summary>
        /// <typeparam name="TKey">The range type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="ranges">The ranges to make covariant.</param>
        /// <remarks>It doesn't actually MAKE them covariant, but how else it can be described?</remarks>
        [NotNull]
        internal static IEnumerable<Range<TKey>> MakeCovariant<TKey, TValue>(
            [NotNull] this IEnumerable<RangeWithValue<TKey, TValue>> ranges) where TKey : IComparable<TKey>
        {
            return ranges;
        }
    }
}