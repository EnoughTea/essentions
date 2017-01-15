using System.Collections.Generic;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="IList{T}" /> </summary>
    public static class ListExtensions
    {
        /// <summary> Swaps the elements with the specified indices in the list. </summary>
        /// <typeparam name="T">List element type.</typeparam>
        /// <param name="self">The list where swapping will occur.</param>
        /// <param name="firstIndex">Index of the first element.</param>
        /// <param name="secondIndex">Index of the second element.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     First or second element index is out of range.
        /// </exception>
        public static void Swap<T>([NotNull] this IList<T> self, int firstIndex, int secondIndex)
        {
            Check.NotNull(self)
                 .GreaterOrEqual(firstIndex, 0, "First element index is out of range.",
                                 nameof(firstIndex))
                 .Less(firstIndex, self.Count, argumentName: "First element index is out of range.")
                 .GreaterOrEqual(secondIndex, 0, "Second element index is out of range.",
                                 nameof(secondIndex))
                 .Less(secondIndex, self.Count, argumentName: "Second element index is out of range.");

            var tmp = self[firstIndex];
            self[firstIndex] = self[secondIndex];
            self[secondIndex] = tmp;
        }
    }
}