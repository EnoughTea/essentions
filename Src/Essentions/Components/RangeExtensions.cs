using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Essentions.Components
{
    /// <summary> Extension methods for <see cref="Range{T}" /> </summary>
    public static class RangeExtensions
    {
        /// <summary> Returns average value for the range. </summary>
        /// <param name="range">The range.</param>
        /// <returns> Average value. </returns>
        public static float Average([NotNull] this Range<int> range)
        {
            Check.NotNull(range);

            return range.Min + (range.Max - range.Min) / 2f;
        }

        /// <summary> Returns an enumerator that iterates through the range. </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through the range.
        /// </returns>
        public static IEnumerator<int> GetEnumerator([NotNull] this Range<int> range)
        {
            Check.NotNull(range);

            for (var i = range.Min; i <= range.Max; i++) yield return i;
        }

        /// <summary> Selects the random value from the given range. </summary>
        /// <param name="rand">The random generator to use.</param>
        /// <param name="range">The range to select from.</param>
        /// <returns> Randomly chosen value belonging to the given range. </returns>
        public static int RandomValue([NotNull] this Range<int> range, [NotNull] Random rand)
        {
            Check.NotNull(range)
                 .NotNull(rand);

            return rand.Next(range.Min, range.Max + 1);
        }
    }
}