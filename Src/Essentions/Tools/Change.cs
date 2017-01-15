using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Essentions.Tools
{
    /// <summary> Contains value changing methods. </summary>
    public static class Change
    {
        /// <summary> Statically casts the specified object using type of the given example object. </summary>
        /// <typeparam name="T">Type of the example object.</typeparam>
        /// <param name="casted">The object to cast.</param>
        /// <param name="targetType">The object whose type will be used for the cast.</param>
        /// <returns>Casted object.</returns>
        [NotNull]
        public static T CastTo<T>([NotNull] object casted, [CanBeNull] T targetType)
        {
            Check.NotNull(casted);

            return (T)casted;
        }

        /// <summary> Finds which numbers is bigger and which is smaller. </summary>
        /// <param name="value1">The first value to test.</param>
        /// <param name="value2">The second value to test.</param>
        /// <param name="max">The bigger value.</param>
        /// <param name="min">The lesser value.</param>
        [CLSCompliant(false)]
        public static void Sort(int value1, int value2, out int min, out int max)
        {
            if (value1 > value2) {
                max = value1;
                min = value2;
            }
            else {
                max = value2;
                min = value1;
            }
        }

        /// <summary> Finds which numbers is bigger and which is smaller. </summary>
        /// <param name="value1">The first value to test.</param>
        /// <param name="value2">The second value to test.</param>
        /// <param name="max">The bigger value.</param>
        /// <param name="min">The lesser value.</param>
        [CLSCompliant(false)]
        public static void Sort(float value1, float value2, out float min, out float max)
        {
            if (value1 > value2) {
                max = value1;
                min = value2;
            }
            else {
                max = value2;
                min = value1;
            }
        }

        /// <summary> Finds which numbers is bigger and which is smaller. </summary>
        /// <param name="value1">The first value to test.</param>
        /// <param name="value2">The second value to test.</param>
        /// <param name="max">The bigger value.</param>
        /// <param name="min">The lesser value.</param>
        [CLSCompliant(false)]
        public static void Sort(double value1, double value2, out double min, out double max)
        {
            if (value1 > value2) {
                max = value1;
                min = value2;
            }
            else {
                max = value2;
                min = value1;
            }
        }

        /// <summary> Converts given marshallable structure into a byte array.</summary>
        /// <param name="target">Marshallable structure to convert.</param>
        /// <returns>Byte representation of the marshallable structure.</returns>
        public static byte[] StructToBytes<T>(T target) where T : struct
        {
            var targetSize = Marshal.SizeOf(target);
            var buffer = new byte[targetSize];
            var ptr = Marshal.AllocHGlobal(targetSize);
            try {
                Marshal.StructureToPtr(target, ptr, true);
                Marshal.Copy(ptr, buffer, 0, targetSize);
            }
            finally {
                Marshal.FreeHGlobal(ptr);
            }

            return buffer;
        }

        /// <summary> Swaps contents of the two storage locations in place. </summary>
        /// <typeparam name="T">Content type.</typeparam>
        /// <param name="left">The first location.</param>
        /// <param name="right">The second location.</param>
        [CLSCompliant(false)]
        public static void Swap<T>(ref T left, ref T right)
        {
            var temp = left;
            left = right;
            right = temp;
        }
    }
}