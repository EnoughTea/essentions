using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Essentions.Components
{
    /// <summary> Represents a range of items, with an associated value. </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    [DataContract(Name = "valRange", IsReference = true, Namespace = "")]
    public sealed class RangeWithValue<TKey, TValue> : Range<TKey>, IEquatable<RangeWithValue<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        /// <summary>Initializes a new instance of the <see cref="RangeWithValue{TKey, TValue}" /> class.</summary>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <param name="value">The value.</param>
        public RangeWithValue(TKey min, TKey max, TValue value)
            : base(min, max)
        {
            Value = value;
        }

        /// <summary> Gets the value associated with this range. </summary>
        [DataMember(Name = "val")]
        public TValue Value { get; }

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var range = obj as RangeWithValue<TKey, TValue>;
            return (range != null) && Equals(range);
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures
        ///     like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked {
                return (base.GetHashCode() * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(Value);
            }
        }

        /// <summary> The equality operator. </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if operands are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(RangeWithValue<TKey, TValue> left, RangeWithValue<TKey, TValue> right)
        {
            return Equals(left, right);
        }

        /// <summary> The inequality operator. </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if operands are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(RangeWithValue<TKey, TValue> left, RangeWithValue<TKey, TValue> right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="RangeWithValue{TKey, TValue}" />,
        ///     is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="RangeWithValue{TKey, TValue}" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="RangeWithValue{TKey, TValue}" /> is equal to this instance;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(RangeWithValue<TKey, TValue> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }
    }
}