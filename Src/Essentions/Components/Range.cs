// Lots of code from Sean Hederman's article at http://codeproject.com/Articles/19028/Building-a-Generic-Range-class
// License unknown, just wrote "have fun with the class".
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Essentions.Components
{
    /// <summary> Represents an inclusive range of items. </summary>
    /// <typeparam name="T">The range type.</typeparam>
    [DataContract(Name = "range", IsReference = true, Namespace = "")]
    public class Range<T> : IEquatable<Range<T>>, IComparable<Range<T>>, IComparable<T>, IComparable
        where T : IComparable<T>
    {
        private static readonly Lazy<Range<T>> _Empty = new Lazy<Range<T>>(() => new Range<T>());

        /// <summary>Gets the empty range.</summary>
        public static Range<T> Empty => _Empty.Value;

        /// <summary>Initializes a new instance of the <see cref="Range{T}"/> class.</summary>
        public Range()
        {
            T zero = (T)typeof(T).Instantiate();
            Check.NotNull(zero);

            Min = Max = zero;
        }

        /// <summary>Initializes a new instance of the <see cref="Range{T}"/> class.</summary>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        public Range([NotNull] T min, [NotNull] T max)
        {
            Check.NotNull(min)
                 .NotNull(max)
                 .LessOrEqual(min, max);

            Min = min;
            Max = max;
        }

        /// <summary> The upper bound of the range. </summary>
        [DataMember(Name = "max", Order = 1)]
        [NotNull]
        public T Max { get; }

        /// <summary> The start of the range. </summary>
        [DataMember(Name = "min", Order = 0)]
        [NotNull]
        public T Min { get; }

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Range<T>)obj);
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures
        ///     like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked {
                return (EqualityComparer<T>.Default.GetHashCode(Max) * 397) ^
                       EqualityComparer<T>.Default.GetHashCode(Min);
            }
        }

        /// <summary> Returns a range which contains the current range, minus <paramref name="value" />. </summary>
        /// <param name="value">The value to complement the range by.</param>
        /// <returns>The complemented range or null if this range contains given range.</returns>
        [CanBeNull]
        public Range<T> Complement([NotNull] Range<T> value)
        {
            if (Contains(value)) return null;

            if (Overlaps(value)) {
                // If value's start and end straddle our start, move our start up to be values end.
                var start = (Min.CompareTo(value.Min) > 0) && (Min.CompareTo(value.Max) < 0) ? value.Max : Min;

                // If value's start and end straddle our end, move our end back down to be values start.
                return (Max.CompareTo(value.Min) > 0) && (Max.CompareTo(value.Max) < 0)
                    ? new Range<T>(start, value.Min)
                    : new Range<T>(start, Max);
            }

            return this;
        }

        /// <summary> Indicates if the range contains <code>value</code>. </summary>
        /// <param name="value">The value to look for.</param>
        /// <returns>true if the range contains <code>value</code>, false otherwise.</returns>
        public bool Contains([NotNull] T value)
        {
            return (Min.CompareTo(value) <= 0) && (Max.CompareTo(value) >= 0);
        }

        /// <summary> Indicates if the range contains <code>value</code>. </summary>
        /// <param name="value">A range to test.</param>
        /// <returns>true if the entire range in <code>value</code> is within this range.</returns>
        public bool Contains([NotNull] Range<T> value)
        {
            return (Min.CompareTo(value.Min) <= 0) && (Max.CompareTo(value.Max) >= 0);
        }

        /// <summary> Returns the range that represents the intersection of this range and <code>value</code>. </summary>
        /// <param name="value">The range to intersect with.</param>
        /// <returns>
        ///     A range that contains the values that are common in both ranges,
        ///     or null if there is no intersection.
        /// </returns>
        public Range<T> Intersect([NotNull] Range<T> value)
        {
            if (!Overlaps(value)) return null;

            var start = Min.CompareTo(value.Min) > 0 ? Min : value.Min;
            return Max.CompareTo(value.Max) < 0 ? new Range<T>(start, Max) : new Range<T>(start, value.Max);
        }

        /// <summary> Indicates if the range is contained by <code>value</code>. </summary>
        /// <param name="value">A range to test.</param>
        /// <returns>true if the entire range is within <code>value</code>.</returns>
        public bool IsContainedBy([NotNull] Range<T> value)
        {
            return value.Contains(this);
        }

        /// <summary> Indicates if this range is contiguous with <code>range</code>. </summary>
        /// <param name="range">The range to check.</param>
        /// <returns>true if the two ranges are contiguous, false otherwise.</returns>
        /// <remarks>Contiguous can mean containing, overlapping, or being next to.</remarks>
        public bool IsContiguousWith([NotNull] Range<T> range)
        {
            if (Contains(range) || range.Contains(this)) return false;

            // Once we remove overlapping and containing, only touching if available
            return Max.Equals(range.Min) || Min.Equals(range.Max);
        }

        /// <summary> Iterates the range from the <see cref="Min"/> using the specified incrementing function,
        /// until incrementing function returns a value greater than <see cref="Max"/>. </summary>
        /// <param name="incrementor">A function which takes a value, and returns the next value.</param>
        /// <returns>The items in the range.</returns>
        [NotNull]
        public IEnumerable<T> Iterate([NotNull] Func<T, T> incrementor)
        {
            yield return Min;
            var item = incrementor(Min);
            while (Max.CompareTo(item) >= 0) {
                yield return item;
                item = incrementor(item);
            }
        }

        /// <summary> Indicates if the range overlaps <code>value</code>. </summary>
        /// <param name="value">A range to test.</param>
        /// <returns>true if any of the range in <code>value</code> is within this range.</returns>
        public bool Overlaps([NotNull] Range<T> value)
        {
            return Contains(value.Min) || Contains(value.Max) || value.Contains(Min) || value.Contains(Max);
        }

        /// <summary> Iterates the range from the <see cref="Max"/> using the specified incrementing function,
        /// until incrementing function returns a value less than <see cref="Min"/>. </summary>
        /// <param name="decrementor">A function which takes a value, and returns the previous value.</param>
        /// <returns>The items in the range.</returns>
        [NotNull]
        public IEnumerable<T> ReverseIterate([NotNull] Func<T, T> decrementor)
        {
            yield return Max;
            var item = decrementor(Max);
            while (CompareTo(item) <= 0) {
                yield return item;
                item = decrementor(item);
            }
        }

        /// <summary> Splits the range into two. </summary>
        /// <param name="position">The position to split the range at.</param>
        /// <returns>The split ranges.</returns>
        [NotNull]
        public IEnumerable<Range<T>> Split([NotNull] T position)
        {
            if (!Contains(position)) yield break;

            if ((Min.CompareTo(position) == 0) || (Max.CompareTo(position) == 0)) {
                // The position is at a boundary, so a split does not happen
                yield return this;
            }
            else {
                yield return new Range<T>(Min, position);
                yield return new Range<T>(position, Max);
            }
        }

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"[{Min}, {Max}]";
        }

        /// <summary> Returns the range that represents the union of this range and <code>value</code>. </summary>
        /// <param name="value">The range to union with.</param>
        /// <returns>A range that contains both ranges.</returns>
        public Range<T> Union([NotNull] Range<T> value)
        {
            // If either one is a subset of the other, then it is the union
            if (Contains(value)) return this;

            if (value.Contains(this)) return value;

            var start = Min.CompareTo(value.Min) < 0 ? Min : value.Min;
            return Max.CompareTo(value.Max) > 0 ? new Range<T>(start, Max) : new Range<T>(start, value.Max);
        }

        /// <summary> The equality operator. </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if operands are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==([CanBeNull] Range<T> left, [CanBeNull] Range<T> right)
        {
            return Equals(left, right);
        }

        /// <summary> The inequality operator. </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if operands are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=([CanBeNull] Range<T> left, [CanBeNull] Range<T> right)
        {
            return !Equals(left, right);
        }

        /// <summary> The intersection operator. </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The intersection of <code>left</code> and <code>right</code>.</returns>
        [CanBeNull]
        public static Range<T> operator &([CanBeNull] Range<T> left, [CanBeNull] Range<T> right)
        {
            if ((left == null) || (right == null)) return null;

            return left.Intersect(right);
        }

        /// <summary> The union operator. </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The union of <code>left</code> and <code>right</code>.</returns>
        [CanBeNull]
        public static Range<T> operator |([CanBeNull] Range<T> left, [CanBeNull] Range<T> right)
        {
            if ((left == null) || (right == null)) return null;

            return left.Union(right);
        }

        /// <summary> The complement operator. </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The complement of <code>left</code> and <code>right</code>.</returns>
        [CanBeNull]
        public static Range<T> operator ^([CanBeNull] Range<T> left, [CanBeNull] Range<T> right)
        {
            if ((left == null) || (right == null)) return null;

            return left.Complement(right);
        }

        /// <summary> Overrides the greater than operator. </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is greater than <code>right</code>, false otherwise.</returns>
        public static bool operator >([CanBeNull] Range<T> left, [CanBeNull] Range<T> right)
        {
            return left?.CompareTo(right) > 0;
        }

        /// <summary>
        ///     Overrides the greater than operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is greater than <code>right</code>, false otherwise.</returns>
        public static bool operator >([CanBeNull] Range<T> left, [CanBeNull] T right)
        {
            return left?.CompareTo(right) > 0;
        }

        /// <summary> Overrides the greater than or equal operator. </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is greater than or equal to <code>right</code>, false otherwise.</returns>
        public static bool operator >=([CanBeNull] Range<T> left, [CanBeNull] Range<T> right)
        {
            return left?.CompareTo(right) >= 0;
        }

        /// <summary> Overrides the greater than or equals operator. </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is greater than or equal to <code>right</code>, false otherwise.</returns>
        public static bool operator >=([CanBeNull] Range<T> left, [CanBeNull] T right)
        {
            return left?.CompareTo(right) >= 0;
        }

        /// <summary> Overrides the less than operator. </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is less than <code>right</code>, false otherwise.</returns>
        public static bool operator <([CanBeNull] Range<T> left, [CanBeNull] Range<T> right)
        {
            return left?.CompareTo(right) < 0;
        }

        /// <summary>
        ///     Overrides the less than operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is less than <code>right</code>, false otherwise.</returns>
        public static bool operator <([CanBeNull] Range<T> left, [CanBeNull] T right)
        {
            return left?.CompareTo(right) < 0;
        }

        /// <summary> Overrides the less than or equal to operator. </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is less than or equal to <code>right</code>, false otherwise.</returns>
        public static bool operator <=([CanBeNull] Range<T> left, [CanBeNull] Range<T> right)
        {
            return left?.CompareTo(right) <= 0;
        }

        /// <summary> Overrides the less than or equals operator. </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is less than or equal to <code>right</code>, false otherwise.</returns>
        public static bool operator <=([CanBeNull] Range<T> left, [CanBeNull] T right)
        {
            return left?.CompareTo(right) <= 0;
        }

        /// <summary>
        ///     Compares the current range minimum with another range or object of the range element type and returns
        ///     an integer that indicates whether the current instance precedes, follows, or occurs in the
        ///     same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">Another range or object of the range element type to compare with this instance.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared.
        ///     The return value has these meanings:
        ///     Less than zero — This instance precedes <paramref name="obj" /> in the sort order.
        ///     Zero — This instance occurs in the same position in the sort order as <paramref name="obj" />.
        ///     Greater than zero — This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        /// <exception cref="InvalidOperationException">Cannot compare to this instance to given object.</exception>
        public int CompareTo([CanBeNull] object obj)
        {
            if (ReferenceEquals(obj, null)) {
                return 1;
            }

            var range = obj as Range<T>;
            if (range != null) {
                return CompareTo(range);
            }

            if (obj is T) {
                var other = (T)obj;
                return CompareTo(other);
            }

            throw new InvalidOperationException("Cannot compare to " + obj);
        }

        /// <summary>
        ///     Compares the current range minimum with another range minimum and returns an integer that
        ///     indicates whether the current instance precedes, follows, or occurs in the same position in the sort
        ///     order as the other object.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared.
        ///     The return value has these meanings:
        ///     Less than zero — This instance precedes <paramref name="other" /> in the sort order.
        ///     Zero — This instance occurs in the same position in the sort order as <paramref name="other" />.
        ///     Greater than zero — This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo([CanBeNull] Range<T> other)
        {
            if (ReferenceEquals(other, null)) return 1;

            return Min.CompareTo(other.Min);
        }

        /// <summary>
        ///     Compares the current range minimum with another object and returns an integer that
        ///     indicates whether the current instance precedes, follows, or occurs in the same position in the sort
        ///     order as the other object.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared.
        ///     The return value has these meanings:
        ///     Less than zero — This instance precedes <paramref name="other" /> in the sort order.
        ///     Zero — This instance occurs in the same position in the sort order as <paramref name="other" />.
        ///     Greater than zero — This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo([CanBeNull] T other)
        {
            if (ReferenceEquals(other, null)) return 1;

            return Min.CompareTo(other);
        }

        /// <summary>Determines whether the specified <see cref="Range{T}" />, is equal to this instance.</summary>
        /// <param name="other">The <see cref="Range{T}" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Range{T}" /> is equal to this instance;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public bool Equals([CanBeNull] Range<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Max, other.Max) &&
                   EqualityComparer<T>.Default.Equals(Min, other.Min);
        }
    }
}