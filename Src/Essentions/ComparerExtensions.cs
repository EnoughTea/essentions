using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary>Extension methods for <see cref="IComparer{T}"/>.</summary>
    public static class ComparerExtensions
    {
        /// <summary>Gets a reversed comparer: it returns the result of original comparison,
        /// but reversing the order of said comparison. Reversing twice in a row does not create a new comparer,
        /// original comparer is returned in that case.</summary>
        /// <typeparam name="T">Type of the instances to compare.</typeparam>
        /// <param name="comparer">The comparer to reverse.</param>
        /// <returns>On first reverse returns an instance of the <see cref="ReverseComparer{T}"/>,
        /// second reverse on it returns original comparer.</returns>
        public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            var alreadyReversed = comparer as ReverseComparer<T>;
            return alreadyReversed?.Original ?? new ReverseComparer<T>(comparer);
        }

        /// <summary>Chains this comparer with another comparer to get a composite sort.</summary>
        /// <typeparam name="T">Type of the instances to compare.</typeparam>
        /// <param name="comparer">The first comparer.</param>
        /// <param name="second">The second comparer.</param>
        /// <returns>Instance of the <see cref="ChainedComparer{T}"/> providing the composite sort.</returns>
        public static IComparer<T> ChainWith<T>(this IComparer<T> comparer, IComparer<T> second)
        {
            return new ChainedComparer<T>(comparer, second);
        }

        /// <summary>Chains this comparer with another comparison to get a composite sort. </summary>
        /// <typeparam name="T">Type of the instances to compare.</typeparam>
        /// <param name="comparer">The first comparer.</param>
        /// <param name="second">The second comparison.</param>
        /// <returns>Instance of the <see cref="ChainedComparer{T}"/> providing the composite sort.</returns>
        public static IComparer<T> ChainWith<T>(this IComparer<T> comparer, Comparison<T> second)
        {
            return new ChainedComparer<T>(comparer, second);
        }
    }

    /// <summary> Chains two existing comparers and applies them in sequence:
    /// sort by first, then sort by second if first returned 0.</summary>
    /// <typeparam name="T">Type of the instances to compare.</typeparam>
    public sealed class ChainedComparer<T> : IComparer<T>
    {
        /// <summary>Initializes a new instance of the <see cref="ChainedComparer{T}"/> class.</summary>
        /// <param name="first">The first comparer to apply.</param>
        /// <param name="secondComparer">The second comparer to apply, if the first one returned 0.</param>
        public ChainedComparer([NotNull] IComparer<T> first, [NotNull] IComparer<T> secondComparer)
        {
            Check.NotNull(first)
                 .NotNull(secondComparer);

            _first = first;
            _secondComparer = secondComparer;
        }

        /// <summary>Initializes a new instance of the <see cref="ChainedComparer{T}"/> class.</summary>
        /// <param name="first">The first comparer to apply.</param>
        /// <param name="secondComparison">The second comparison to apply, if the first one returned 0.</param>
        public ChainedComparer([NotNull] IComparer<T> first, [NotNull] Comparison<T> secondComparison)
        {
            Check.NotNull(first)
                 .NotNull(secondComparison);

            _first = first;
            _secondComparison = secondComparison;
        }

        [NotNull]
        private readonly IComparer<T> _first;
        [CanBeNull]
        private readonly IComparer<T> _secondComparer;
        [CanBeNull]
        private readonly Comparison<T> _secondComparison;

        /// <summary>Returns the result of applying current comparers in sequence: sort by first,
        /// then sort by second if first returned 0.</summary>
        int IComparer<T>.Compare(T x, T y)
        {
            int firstResult = _first.Compare(x, y);
            if (firstResult == 0) {
                if (_secondComparer != null) {
                    return _secondComparer.Compare(x, y);
                }

                Debug.Assert(_secondComparison != null);
                return _secondComparison(x, y);
            }

            return firstResult;
        }
    }

    /// <summary> Simple comparer that reverses the passed original comparer.</summary>
    /// <typeparam name="T">Type of the instances to compare.</typeparam>
    public sealed class ReverseComparer<T> : IComparer<T>
    {
        /// <summary>Initializes a new instance of the <see cref="ReverseComparer{T}"/> class.</summary>
        /// <param name="comparer">The original comparer to use for comparisons.</param>
        public ReverseComparer([NotNull] IComparer<T> comparer)
        {
            Check.NotNull(comparer);

            Original = comparer;
        }

        /// <summary>Gets the comparer which is reversed. </summary>
        [NotNull]
        public IComparer<T> Original { get; }

        /// <summary>Returns the result of original comparison, but reversing the order of said comparison.</summary>
        public int Compare(T x, T y)
        {
            return Original.Compare(y, x);
        }
    }
}
