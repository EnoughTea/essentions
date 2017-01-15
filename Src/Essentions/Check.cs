using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Used for parameter checking, throws when assertion is false. </summary>
    public static class Check
    {
        #region Booleans

        /// <exception cref="ArgumentException">Value is false.</exception>
        [DebuggerStepThrough]
        public static CheckContinue True(bool value,
                                         [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.True(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentException">Value is null or false.</exception>
        [DebuggerStepThrough]
        public static CheckContinue True(bool? value,
                                         [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.True(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentException">Value is true.</exception>
        [DebuggerStepThrough]
        public static CheckContinue False(bool value,
                                          [CanBeNull] string errorMessage = null,
                                          [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.False(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentException">Value is null or true.</exception>
        [DebuggerStepThrough]
        public static CheckContinue False(bool? value,
                                          [CanBeNull] string errorMessage = null,
                                          [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.False(value, errorMessage, argumentName);
        }

        #endregion

        #region InRange

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Value is null or (value &lt; min || value &gt; max).
        /// </exception>
        [DebuggerStepThrough]
        public static CheckContinue InRange<T>(T value, T min, T max, [CanBeNull] string errorMessage = null,
                                               [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            return CheckContinue.Next.InRange(value, min, max, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Value is null or (value &lt;= min || value &gt;= max).
        /// </exception>
        [DebuggerStepThrough]
        public static CheckContinue InRangeExclusive<T>(T value, T min, T max, [CanBeNull] string errorMessage = null,
                                                        [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            return CheckContinue.Next.InRangeExclusive(value, min, max, errorMessage, argumentName);
        }

        #endregion

        #region Negative

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Negative(float value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Negative(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Negative(double value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Negative(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Negative(decimal value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Negative(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Negative(int value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Negative(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Negative(long value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Negative(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Negative(short value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Negative(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Negative(BigInteger value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Negative(value, errorMessage, argumentName);
        }

        #endregion

        #region Positive

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Positive(float value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Positive(double value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Positive(decimal value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Positive(int value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public static CheckContinue Positive(uint value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Positive(long value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public static CheckContinue Positive(ulong value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Positive(short value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public static CheckContinue Positive(ushort value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Positive(BigInteger value, [CanBeNull] string errorMessage = null,
                                             [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Positive(value, errorMessage, argumentName);
        }

        #endregion

        #region Zero

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Zero(float value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Zero(double value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Zero(decimal value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Zero(int value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public static CheckContinue Zero(uint value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Zero(long value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public static CheckContinue Zero(ulong value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Zero(short value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public static CheckContinue Zero(ushort value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Zero(BigInteger value, [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.Zero(value, errorMessage, argumentName);
        }

        #endregion

        #region NotZero

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NonZero(float value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NonZero(double value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NonZero(decimal value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NonZero(int value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public static CheckContinue NonZero(uint value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NonZero(long value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public static CheckContinue NonZero(ulong value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NonZero(short value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public static CheckContinue NonZero(ushort value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NonZero(BigInteger value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NonZero(value, errorMessage, argumentName);
        }

        #endregion

        #region Greater and less

        /// <exception cref="ArgumentOutOfRangeException">Value is null or value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Greater<T>(T value, T min, [CanBeNull] string errorMessage = null,
                                               [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            return CheckContinue.Next.Greater(value, min, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is null or value &lt; 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue GreaterOrEqual<T>(T value, T min, [CanBeNull] string errorMessage = null,
                                                      [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            return CheckContinue.Next.GreaterOrEqual(value, min, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is null or value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Less<T>(T value, T max, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            return CheckContinue.Next.Less(value, max, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is null or value &gt; 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue LessOrEqual<T>(T value, T max, [CanBeNull] string errorMessage = null,
                                                   [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            return CheckContinue.Next.LessOrEqual(value, max, errorMessage, argumentName);
        }

        #endregion

        #region NotNan

        /// <exception cref="ArgumentOutOfRangeException">Value is NaN.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotNaN(double value, [CanBeNull] string errorMessage = null,
                                           [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NotNaN(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is not null and value is NaN.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotNaN(double? value, [CanBeNull] string errorMessage = null,
                                           [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NotNaN(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is NaN.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotNaN(float value, [CanBeNull] string errorMessage = null,
                                           [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NotNaN(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is not null and value is NaN.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotNaN(float? value, [CanBeNull] string errorMessage = null,
                                           [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NotNaN(value, errorMessage, argumentName);
        }

        #endregion

        #region NotNull...

        /// <exception cref="ArgumentNullException">Value is null.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotNull([CanBeNull] object value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NotNull(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentNullException">Value is null.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotNull<T>([CanBeNull] T value, [CanBeNull] string errorMessage = null,
                                               [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NotNull(value, errorMessage, argumentName);
        }


        /// <exception cref="ArgumentNullException">Value is null.</exception>
        /// <exception cref="ArgumentException">Value's length == 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotNullOrEmpty([CanBeNull] string value, [CanBeNull] string errorMessage = null,
                                                   [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NotNullOrEmpty(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentNullException">Value is null.</exception>
        /// <exception cref="ArgumentException">Value's length == 0.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotNullOrEmpty([CanBeNull] IEnumerable value,
                                                   [CanBeNull] string errorMessage = null,
                                                   [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NotNullOrEmpty(value, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentNullException">Value is null.</exception>
        /// <exception cref="ArgumentException">Value is empty or all whitespace.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotNullOrWhiteSpace([CanBeNull] string value,
                                                        [CanBeNull] string errorMessage = null,
                                                        [CanBeNull] string argumentName = null)
        {
            return CheckContinue.Next.NotNullOrWhiteSpace(value, errorMessage, argumentName);
        }

        #endregion

        #region Equals

        /// <exception cref="ArgumentException">Value != expected.</exception>
        [DebuggerStepThrough]
        public static CheckContinue Equals<T>(T value, T expected, string errorMessage = null,
                                              string argumentName = null)
        {
            return CheckContinue.Next.Equals(value, expected, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentException">Value == expected.</exception>
        [DebuggerStepThrough]
        public static CheckContinue NotEqual<T>(T value, T expected, string errorMessage = null,
                                                string argumentName = null)
        {
            return CheckContinue.Next.NotEqual(value, expected, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentException">
        ///     <paramref name="first" /> and <paramref name="second" /> do not have the same number of elements.
        /// </exception>
        [DebuggerStepThrough]
        public static CheckContinue EqualCount(IEnumerable first, IEnumerable second, string errorMessage = null,
                                               string argumentName = null)
        {
            return CheckContinue.Next.EqualCount(first, second, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentException">
        ///     <paramref name="first" /> and <paramref name="second" /> do not have the same number of elements.
        /// </exception>
        [DebuggerStepThrough]
        public static CheckContinue EqualCount<TFirst, TSecond>(IEnumerable<TFirst> first, IEnumerable<TSecond> second,
                                                                string errorMessage = null, string argumentName = null)
        {
            return CheckContinue.Next.EqualCount(first, second, errorMessage, argumentName);
        }

        /// <exception cref="ArgumentException">
        ///     <paramref name="first" /> and <paramref name="second" /> have the same number of elements.
        /// </exception>
        [DebuggerStepThrough]
        public static CheckContinue NotEqualCount(IEnumerable first, IEnumerable second, string errorMessage = null,
                                                  string argumentName = null)
        {
            return CheckContinue.Next.NotEqualCount(first, second, errorMessage, argumentName);
        }


        /// <exception cref="ArgumentException">
        ///     <paramref name="first" /> and <paramref name="second" /> have the same number of elements.
        /// </exception>
        [DebuggerStepThrough]
        public static CheckContinue NotEqualCount<TFirst, TSecond>(IEnumerable<TFirst> first,
                                                                   IEnumerable<TSecond> second,
                                                                   string errorMessage = null,
                                                                   string argumentName = null)
        {
            return CheckContinue.Next.NotEqualCount(first, second, errorMessage, argumentName);
        }

        #endregion
    }
}