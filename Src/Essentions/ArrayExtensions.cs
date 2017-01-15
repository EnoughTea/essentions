using System;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extensions methods for <see cref="Array" />. </summary>
    public static class ArrayExtensions
    {
        /// <summary> Concatenates two arrays into a new array. </summary>
        /// <typeparam name="T">The type of the elements of the input arrays.</typeparam>
        /// <param name="array">The first array to concatenate.</param>
        /// <param name="other">The array to concatenate to the first array.</param>
        /// <returns>An array that contains the concatenated elements of the two input arrays.</returns>
        [NotNull]
        public static T[] Concat<T>([NotNull] this T[] array, [CanBeNull] T[] other)
        {
            Check.NotNull(array);

            T[] result;
            if ((other != null) && (other.Length > 0)) {
                result = new T[array.Length + other.Length];
                array.CopyTo(result, 0);
                other.CopyTo(result, array.Length);
            }
            else {
                result = new T[array.Length];
                array.CopyTo(result, 0);
            }

            return result;
        }

        /// <summary> Fills the specified 2D array using passed function. </summary>
        /// <typeparam name="T">The type of the elements of the array to fill.</typeparam>
        /// <param name="array">The array to fill.</param>
        /// <param name="filling">The filling function: (x, y, old element) => new element.</param>
        public static void Fill<T>([NotNull] this T[,] array, [NotNull] Func<int, int, T, T> filling)
        {
            Check.NotNull(array)
                 .NotNull(filling);

            for (var x = array.GetLowerBound(0); x <= array.GetUpperBound(0); x++)
                for (var y = array.GetLowerBound(1); y <= array.GetUpperBound(1); y++)
                    array[x, y] = filling(x, y, array[x, y]);
        }

        /// <summary> Fills the specified 3D array using passed function. </summary>
        /// <typeparam name="T">The type of the elements of the array to fill.</typeparam>
        /// <param name="array">The array to fill.</param>
        /// <param name="filling">The filling function: (x, y, z, old element) => new element.</param>
        public static void Fill<T>([NotNull] this T[,,] array, [NotNull] Func<int, int, int, T, T> filling)
        {
            Check.NotNull(array)
                 .NotNull(filling);

            for (var x = array.GetLowerBound(0); x <= array.GetUpperBound(0); x++)
                for (var y = array.GetLowerBound(1); y <= array.GetUpperBound(1); y++)
                    for (var z = array.GetLowerBound(2); z <= array.GetUpperBound(2); z++)
                        array[x, y, z] = filling(x, y, z, array[x, y, z]);
        }

        /// <summary> Slices array into array of arrays of length up to <paramref name="sliceLength" />. </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The array to slice.</param>
        /// <param name="sliceLength">Length of the slice.</param>
        /// <returns>Jagged array of sliced arrays.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Slice length is &lt; 0.</exception>
        [NotNull]
        public static T[][] JaggedSlice<T>([NotNull] this T[] array, int sliceLength)
        {
            Check.NotNull(array)
                 .Positive(sliceLength);

            // Calculate total number of sliced arrays.
            var segments = array.Length / sliceLength;
            var last = array.Length % sliceLength; // partial slice.
            var totalSegments = segments + (last == 0 ? 0 : 1);

            var result = new T[totalSegments][];
            for (var i = 0; i < segments; i++) {
                // Fill full-sized slices...
                var item = result[i] = new T[sliceLength];
                var sourceIndex = array.GetLowerBound(0) + i * sliceLength;
                Array.Copy(array, sourceIndex, item, 0, sliceLength);
            }

            if (last > 0) {
                // ... and last partial slice, if present.
                var item = result[totalSegments - 1] = new T[last];
                var sourceIndex = array.GetLowerBound(0) + segments * sliceLength;
                Array.Copy(array, sourceIndex, item, 0, last);
            }

            return result;
        }

        /// <summary> Converts 2D array to a jagged array and makes indexing zero-based. </summary>
        /// <typeparam name="T">The type of the elements of the input array.</typeparam>
        /// <param name="array">The multidimensional array to convert.</param>
        /// <returns>Jagged array.</returns>
        [NotNull]
        public static T[][] ToJaggedArray<T>([NotNull] this T[,] array)
        {
            Check.NotNull(array);

            var firstMin = array.GetLowerBound(0);
            var firstMax = array.GetUpperBound(0);
            var rows = firstMax - firstMin + 1;

            var secondMin = array.GetLowerBound(1);
            var secondMax = array.GetUpperBound(1);
            var cols = secondMax - secondMin + 1;

            var result = new T[rows][];
            for (var row = 0; row < rows; row++) {
                result[row] = new T[cols];
                for (var col = 0; col < cols; col++) result[row][col] = array[row + firstMin, col + secondMin];
            }

            return result;
        }

        /// <summary> Converts 3D array to a jagged array and makes indexing zero-based. </summary>
        /// <typeparam name="T">The type of the elements of the input array.</typeparam>
        /// <param name="array">The multidimensional array to convert.</param>
        /// <returns>Jagged array.</returns>
        [NotNull]
        public static T[][][] ToJaggedArray<T>([NotNull] this T[,,] array)
        {
            Check.NotNull(array);

            var firstMin = array.GetLowerBound(0);
            var firstMax = array.GetUpperBound(0);
            var rows = firstMax - firstMin + 1;

            var secondMin = array.GetLowerBound(1);
            var secondMax = array.GetUpperBound(1);
            var cols = secondMax - secondMin + 1;

            var thirdMin = array.GetLowerBound(2);
            var thirdMax = array.GetUpperBound(2);
            var depths = thirdMax - thirdMin + 1;

            var result = new T[rows][][];
            for (var row = 0; row < rows; row++) {
                result[row] = new T[cols][];
                for (var col = 0; col < cols; col++) {
                    result[row][col] = new T[depths];
                    for (var depth = 0; depth < depths; depth++)
                        result[row][col][depth] = array[row + firstMin, col + secondMin, depth + thirdMin];
                }
            }

            return result;
        }
    }
}