using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Essentions.Tools;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Used to provide fluency for <see cref="Check" /> class. </summary>
    public sealed class CheckContinue
    {
        internal static readonly CheckContinue Next = new CheckContinue();

        private CheckContinue()
        {
        }

        /// <summary>Determines whether the specified enumerables have the same size.</summary>
        /// <returns><c>true</c> if the specified enumerables have the same size; otherwise, <c>false</c>.</returns>
        private static bool AreSameCount<TFirst, TSecond>([CanBeNull] IEnumerable<TFirst> first,
                                                          [CanBeNull] IEnumerable<TSecond> second)
        {
            if (first == null) return second == null;

            if (second == null) return false;

            int firstCount;
            int secondCount;
            var firstGenericCollection = first as ICollection<TFirst>;
            var secondGenericCollection = second as ICollection<TSecond>;

            if (firstGenericCollection != null) {
                firstCount = firstGenericCollection.Count;
            }
            else {
                var firstCollection = first as ICollection;
                firstCount = firstCollection?.Count ?? first.Count();
            }

            if (secondGenericCollection != null) {
                secondCount = secondGenericCollection.Count;
            }
            else {
                var secondCollection = first as ICollection;
                secondCount = secondCollection?.Count ?? second.Count();
            }

            return firstCount == secondCount;
        }

        /// <summary>Determines whether the specified enumerables have the same size.</summary>
        /// <returns><c>true</c> if the specified enumerables have the same size; otherwise, <c>false</c>.</returns>
        private static bool AreSameCount([CanBeNull] IEnumerable first, [CanBeNull] IEnumerable second)
        {
            if (first == null) return second == null;

            if (second == null) return false;

            int firstCount;
            int secondCount;
            var firstGenericCollection = first as ICollection;
            var secondGenericCollection = second as ICollection;

            firstCount = firstGenericCollection?.Count ?? first.Count();
            secondCount = secondGenericCollection?.Count ?? second.Count();

            return firstCount == secondCount;
        }

        /// <summary>Determines whether the specified enumerable is empty.</summary>
        /// <returns><c>true</c> if the specified enumerable is empty; otherwise, <c>false</c>.</returns>
        private static bool IsEmpty([NotNull] IEnumerable enumerable)
        {
            var collection = enumerable as ICollection;
            if (collection != null) return collection.Count == 0;

            foreach (var _ in enumerable) return false;

            return true;
        }

        #region Booleans

        /// <exception cref="ArgumentException">Value is false.</exception>
        [DebuggerStepThrough]
        public CheckContinue True(bool value,
                                  [CanBeNull] string errorMessage = null, [CanBeNull] string argumentName = null)
        {
            if (!value) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentException">Value is null or false.</exception>
        [DebuggerStepThrough]
        public CheckContinue True([CanBeNull] bool? value,
                                  [CanBeNull] string errorMessage = null, [CanBeNull] string argumentName = null)
        {
            if ((value == null) || !value.Value) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentException">Value is true.</exception>
        [DebuggerStepThrough]
        public CheckContinue False(bool value,
                                   [CanBeNull] string errorMessage = null, [CanBeNull] string argumentName = null)
        {
            if (value) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentException">Value is null or true.</exception>
        [DebuggerStepThrough]
        public CheckContinue False([CanBeNull] bool? value,
                                   [CanBeNull] string errorMessage = null, [CanBeNull] string argumentName = null)
        {
            if ((value == null) || value.Value) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        #endregion

        #region InRange

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Value is null or (value &lt; min || value &gt; max).
        /// </exception>
        [DebuggerStepThrough]
        public CheckContinue InRange<T>(T value, T min, T max, [CanBeNull] string errorMessage = null,
                                        [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            if (!Values.NotNull(value) || (value.CompareTo(min) < 0) || (value.CompareTo(max) > 0))
                throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Value is null or (value &lt;= min || value &gt;= max).
        /// </exception>
        [DebuggerStepThrough]
        public CheckContinue InRangeExclusive<T>([CanBeNull] T value, [CanBeNull] T min, [CanBeNull] T max,
                                                 [CanBeNull] string errorMessage = null,
                                                 [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            if (!Values.NotNull(value) || (value.CompareTo(min) <= 0) || (value.CompareTo(max) >= 0))
                throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        #endregion

        #region Negative

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Negative(float value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value >= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Negative(double value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value >= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Negative(decimal value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value >= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Negative(int value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value >= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Negative(long value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value >= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Negative(short value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value >= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Negative(BigInteger value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value >= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        #endregion

        #region Positive

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Positive(float value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Positive(double value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Positive(decimal value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Positive(int value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public CheckContinue Positive(uint value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Positive(long value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public CheckContinue Positive(ulong value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Positive(short value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public CheckContinue Positive(ushort value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Positive(BigInteger value, [CanBeNull] string errorMessage = null,
                                      [CanBeNull] string argumentName = null)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        #endregion

        #region Zero

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Zero(float value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Zero(double value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Zero(decimal value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Zero(int value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public CheckContinue Zero(uint value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Zero(long value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public CheckContinue Zero(ulong value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Zero(short value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public CheckContinue Zero(ushort value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value != 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Zero(BigInteger value, [CanBeNull] string errorMessage = null,
                                  [CanBeNull] string argumentName = null)
        {
            if (value != 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        #endregion

        #region NotZero

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue NonZero(float value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue NonZero(double value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue NonZero(decimal value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue NonZero(int value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public CheckContinue NonZero(uint value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue NonZero(long value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public CheckContinue NonZero(ulong value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue NonZero(short value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [CLSCompliant(false)]
        [DebuggerStepThrough]
        public CheckContinue NonZero(ushort value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value == 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue NonZero(BigInteger value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        #endregion

        #region Greater and less

        /// <exception cref="ArgumentOutOfRangeException">Value is null or value &lt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Greater<T>([CanBeNull] T value, [CanBeNull] T min, [CanBeNull] string errorMessage = null,
                                        [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            if (!Values.NotNull(value) || (value.CompareTo(min) <= 0))
                throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is null or value &lt; 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue GreaterOrEqual<T>([CanBeNull] T value, [CanBeNull] T min,
                                               [CanBeNull] string errorMessage = null,
                                               [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            if (!Values.NotNull(value) || (value.CompareTo(min) < 0))
                throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is null or value &gt;= 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue Less<T>([CanBeNull] T value, [CanBeNull] T max, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            if (!Values.NotNull(value) || (value.CompareTo(max) >= 0))
                throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is null or value &gt; 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue LessOrEqual<T>([CanBeNull] T value, [CanBeNull] T max,
                                            [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
            where T : IComparable<T>
        {
            if (!Values.NotNull(value) || (value.CompareTo(max) > 0))
                throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        #endregion

        #region NotNan

        /// <exception cref="ArgumentOutOfRangeException">Value is NaN.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotNaN(double value, [CanBeNull] string errorMessage = null,
                                    [CanBeNull] string argumentName = null)
        {
            if (double.IsNaN(value)) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is not null and value is NaN.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotNaN([CanBeNull] double? value, [CanBeNull] string errorMessage = null,
                                    [CanBeNull] string argumentName = null)
        {
            if (value.HasValue && double.IsNaN(value.Value))
                throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is NaN.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotNaN(float value, [CanBeNull] string errorMessage = null,
                                    [CanBeNull] string argumentName = null)
        {
            if (float.IsNaN(value)) throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentOutOfRangeException">Value is not null and value is NaN.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotNaN([CanBeNull] float? value, [CanBeNull] string errorMessage = null,
                                    [CanBeNull] string argumentName = null)
        {
            if (value.HasValue && float.IsNaN(value.Value))
                throw new ArgumentOutOfRangeException(argumentName, value, errorMessage);

            return Next;
        }

        #endregion

        #region NotNull...

        /// <exception cref="ArgumentNullException">Value is null.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotNull([CanBeNull] object value, [CanBeNull] string errorMessage = null,
                                     [CanBeNull] string argumentName = null)
        {
            if (!Values.NotNull(value)) throw new ArgumentNullException(argumentName, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentNullException">Value is null.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotNull<T>([CanBeNull] T value, [CanBeNull] string errorMessage = null,
                                        [CanBeNull] string argumentName = null)
        {
            if (!Values.NotNull(value)) throw new ArgumentNullException(argumentName, errorMessage);

            return Next;
        }

        /// <exception cref="ArgumentNullException">Value is null.</exception>
        /// <exception cref="ArgumentException">Value's length == 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotNullOrEmpty([CanBeNull] string value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            if (value == null) throw new ArgumentNullException(argumentName, errorMessage);

            if (value.Length == 0) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentNullException">Value is null.</exception>
        /// <exception cref="ArgumentException">Value's count == 0.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotNullOrEmpty([CanBeNull] IEnumerable value, [CanBeNull] string errorMessage = null,
                                            [CanBeNull] string argumentName = null)
        {
            if (value == null) throw new ArgumentNullException(argumentName, errorMessage);

            if (IsEmpty(value)) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentNullException">Value is null.</exception>
        /// <exception cref="ArgumentException">Value is empty or all whitespace.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotNullOrWhiteSpace([CanBeNull] string value, [CanBeNull] string errorMessage = null,
                                                 [CanBeNull] string argumentName = null)
        {
            if (value == null) throw new ArgumentNullException(argumentName, errorMessage);

            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        #endregion

        #region Equals

        /// <exception cref="ArgumentException">Value != <paramref name="expected" />.</exception>
        [DebuggerStepThrough]
        public CheckContinue Equals<T>([CanBeNull] T value, [CanBeNull] T expected,
                                       [CanBeNull] string errorMessage = null, [CanBeNull] string argumentName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(value, expected))
                throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentException">Value == <paramref name="expected" />.</exception>
        [DebuggerStepThrough]
        public CheckContinue NotEqual<T>([CanBeNull] T value, [CanBeNull] T expected,
                                         [CanBeNull] string errorMessage = null,
                                         [CanBeNull] string argumentName = null)
        {
            if (EqualityComparer<T>.Default.Equals(value, expected))
                throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentException">
        ///     <paramref name="first" /> and <paramref name="second" /> do not have the same number of elements.
        /// </exception>
        [DebuggerStepThrough]
        public CheckContinue EqualCount([CanBeNull] IEnumerable first, [CanBeNull] IEnumerable second,
                                        [CanBeNull] string errorMessage = null, [CanBeNull] string argumentName = null)
        {
            if (!AreSameCount(first, second)) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentException">
        ///     <paramref name="first" /> and <paramref name="second" /> have the same number of elements.
        /// </exception>
        [DebuggerStepThrough]
        public CheckContinue NotEqualCount([CanBeNull] IEnumerable first, [CanBeNull] IEnumerable second,
                                           [CanBeNull] string errorMessage = null,
                                           [CanBeNull] string argumentName = null)
        {
            if (AreSameCount(first, second)) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentException">
        ///     <paramref name="first" /> and <paramref name="second" /> do not have the same number of elements.
        /// </exception>
        [DebuggerStepThrough]
        public CheckContinue EqualCount<TFirst, TSecond>([CanBeNull] IEnumerable<TFirst> first,
                                                         [CanBeNull] IEnumerable<TSecond> second,
                                                         [CanBeNull] string errorMessage = null,
                                                         [CanBeNull] string argumentName = null)
        {
            if (!AreSameCount(first, second)) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        /// <exception cref="ArgumentException">
        ///     <paramref name="first" /> and <paramref name="second" /> have the same number of elements.
        /// </exception>
        [DebuggerStepThrough]
        public CheckContinue NotEqualCount<TFirst, TSecond>([CanBeNull] IEnumerable<TFirst> first,
                                                            [CanBeNull] IEnumerable<TSecond> second,
                                                            [CanBeNull] string errorMessage = null,
                                                            [CanBeNull] string argumentName = null)
        {
            if (AreSameCount(first, second)) throw new ArgumentException(errorMessage, argumentName);

            return Next;
        }

        #endregion
    }
}