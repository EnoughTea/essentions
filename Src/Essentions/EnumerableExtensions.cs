using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Essentions.Tools;
using JetBrains.Annotations;

// ReSharper disable PossibleMultipleEnumeration

namespace Essentions
{
    /// <summary> Extension method for <see cref="IEnumerable{T}" />. </summary>
    public static class EnumerableExtensions
    {
        /// <summary> Calculates cartesian product for the specified sets. </summary>
        /// <remarks>
        ///     The Cartesian product is the result of crossing members of each set with one another.
        ///     <example>
        ///         Cartesian product of { {0, 1}, {1, 2, 3} } would be
        ///         { {0, 1}, {0, 2}, {0, 3 }, {1, 1}, {1, 2}, {1, 3} }.
        ///     </example>
        /// </remarks>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The sequence of sets to cross with each other.</param>
        /// <returns> A product set calculated from multiple sets. </returns>
        [NotNull]
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>([NotNull] this IEnumerable<IEnumerable<T>> self)
        {
            Check.NotNull(self);

            IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
            var any = false;
            foreach (var enumerable in self) {
                any = true;
                result = result.SelectMany(accseq => enumerable, (accseq, item) => accseq.Concat(new[] { item }));
            }

            return any ? result : Enumerable.Empty<IEnumerable<T>>();
        }

        /// <summary>
        ///     Performs a type-safe cast of the source sequence elements to the destination type with
        ///     allowance of explicit/implicit conversion operators.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TDest">The destination type.</typeparam>
        /// <param name="self">The elements to convert.</param>
        /// <returns> Casted elements. </returns>
        public static IEnumerable<TDest> CastWithOperators<TSource, TDest>([NotNull] this IEnumerable<TSource> self)
        {
            Check.NotNull(self);

            foreach (var element in self) {
                yield return Source<TSource>.CastCache<TDest>.Cast(element);
            }
        }

        /// <summary>Counts elements in the specified sequence.</summary>
        /// <param name="sequence">The elements to count.</param>
        /// <returns>Count of the elements in the sequence.</returns>
        public static int Count([NotNull] this IEnumerable sequence)
        {
            Check.NotNull(sequence);

            var collection = sequence as ICollection;
            if (collection != null) return collection.Count;

            var count = 0;
            Values.Using(sequence.GetEnumerator(), enumerator => {
                             while (enumerator.MoveNext()) count++;
                         });
            return count;
        }

        /// <summary> Gets a hash code for a given sequence that takes into account the sequence's contents. </summary>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The sequence for which to compute a hash code.</param>
        /// <returns>The resulting hash code.</returns>
        public static int CalculateHashCode<T>([NotNull] this IEnumerable<T> self)
        {
            Check.NotNull(self);

            var hash = 0;
            var isValueType = typeof(T).GetTypeInfo().IsValueType;
            foreach (var element in self)
                if (isValueType) {
                    hash = (hash * 397) ^ element.GetHashCode();
                }
                else {
                    if (!ReferenceEquals(element, null)) hash = (hash * 397) ^ element.GetHashCode();
                }

            return hash;
        }

        /// <summary> Returns the index of the first item that matches the predicate. </summary>
        /// <remarks> If you're just getting an index so that you can call .Skip, take a look at .SkipWhile.</remarks>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The collection to search.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns> The index of the first matching item, or -1 if none matched. </returns>
        public static int IndexOfFirst<T>([NotNull] this IEnumerable<T> self, [NotNull] Func<T, bool> predicate)
        {
            Check.NotNull(self)
                 .NotNull(predicate);

            var index = 0;
            foreach (var item in self) {
                if (predicate(item)) return index;

                index++;
            }

            return -1;
        }

        /// <summary> Inserts an item between each pair of elements in a sequence. </summary>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The source sequence.</param>
        /// <param name="item">The item to be inserted in the sequence.</param>
        /// <returns>A sequence of the original items interleaved with the added item.</returns>
        [NotNull]
        public static IEnumerable<T> Interleave<T>([NotNull] this IEnumerable<T> self, [CanBeNull] T item)
        {
            Check.NotNull(self);

            using (var enumerator = self.GetEnumerator()) {
                // Empty sequences cannot be interleaved.
                if (!enumerator.MoveNext()) yield break;
                // Otherwise return current element and, if there is a next element, insert the item before the next.
                while (true) {
                    yield return enumerator.Current;
                    if (enumerator.MoveNext()) yield return item;
                    else break;
                }
            }
        }

        /// <summary>
        ///     Checks if two sequences contain the same elements in any order,
        ///     { 1, 2, 3, 1 } is equivalent to { 2, 1, 1, 3}. Optionally allows element duplicates,
        ///     so { 1, 2, 2 } would be equivalent to { 1, 1, 2 } and { 1, 2 }.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="self">First sequence.</param>
        /// <param name="other">Second sequence.</param>
        /// <param name="ignoreDuplicates">
        ///     If set to true, will allow multiple occurences of the same element,
        ///     e.g. { 1, 2, 2 } would be equal to { 1, 1, 2 }; and { 1, 2, 2, 1 } would be equal to { 2, 1 }.
        /// </param>
        /// <returns>
        ///     <c>true</c> sequences contain the same elements in any order, duplicates do not matter;
        ///     false otherwise.
        /// </returns>
        public static bool Equivalent<T>([CanBeNull] this IEnumerable<T> self, [CanBeNull] IEnumerable<T> other,
                                         bool ignoreDuplicates = false)
        {
            // Short-circuit obvious differences
            if (self == null) return other == null;

            if (other == null) return false;

            if (ReferenceEquals(self, other)) return true;
            // Short-circuit by count for collections, since they know their count.
            var firstCollection = self as ICollection<T>;
            var secondCollection = other as ICollection<T>;
            if ((firstCollection != null) && (secondCollection != null))
                if (!ignoreDuplicates) {
                    if (firstCollection.Count != secondCollection.Count) {
                        return false;
                    }

                    if (firstCollection.Count == 0) {
                        return true;
                    }
                }
                else {
                    if (firstCollection.Count == 0) {
                        return secondCollection.Count == 0;
                    }

                    if (secondCollection.Count == 0) {
                        return false;
                    }
                }
            // Time to calculate, it is O(n).
            return !AreSequencesDifferent(self, other, !ignoreDuplicates);
        }

        ///// <summary> Selects types marked with the specified attribute. </summary>
        ///// <param name="self">Types to check.</param>
        ///// <param name="attributeType">Attribute to search for.</param>
        ///// <param name="inherit">True to check inheritance.</param>
        ///// <returns>Types marked with specified attribute.</returns>
        //[NotNull]
        //public static IEnumerable<Type> MarkedWith([NotNull] this IEnumerable<Type> self,
        //    [NotNull] Type attributeType,
        //    bool inherit)
        //{
        //    Check.NotNull(self);

        //    foreach (var type in self) {
        //        if ((type != null) && type.GetTypeInfo().IsDefined(attributeType, inherit)) {
        //            yield return type;
        //        }
        //    }
        //}

        /// <summary> Selects elements which type is marked with the specified attribute. </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="self">Elements to check.</param>
        /// <param name="attributeType">Attribute to search for.</param>
        /// <param name="inherit">True to check inheritance.</param>
        /// <returns>Elements with types marked with specified attribute.</returns>
        [NotNull]
        public static IEnumerable<T> MarkedWith<T>([NotNull] this IEnumerable<T> self,
                                                   [NotNull] Type attributeType,
                                                   bool inherit = false)
        {
            Check.NotNull(self)
                 .NotNull(attributeType);

            foreach (var element in self) {
                if (element == null) continue;

                var typeInfo = element is Type ? (element as Type).GetTypeInfo() : element.GetType().GetTypeInfo();
                if (typeInfo.IsDefined(attributeType, inherit)) yield return element;
            }
        }

        /// <summary> Like Zip which can be used for sequences of different length. </summary>
        [NotNull]
        public static IEnumerable<TResult> Merge<TFirst, TSecond, TResult>(
            [NotNull] this IEnumerable<TFirst> first,
            [NotNull] IEnumerable<TSecond> second,
            [NotNull] Func<TFirst, TSecond, TResult> operation)
        {
            Check.NotNull(first)
                 .NotNull(second)
                 .NotNull(operation);

            using (var iter1 = first.GetEnumerator()) {
                using (var iter2 = second.GetEnumerator()) {
                    while (iter1.MoveNext())
                        if (iter2.MoveNext()) yield return operation(iter1.Current, iter2.Current);
                        else yield return operation(iter1.Current, default(TSecond));

                    while (iter2.MoveNext()) yield return operation(default(TFirst), iter2.Current);
                }
            }
        }

        /// <summary>
        ///     Executes the given action using each element in the sequence as action parameter.
        ///     <b>Sequence itself does not change</b>. Do not mutate sequence in the <paramref name="action" /> method!
        /// </summary>
        /// <remarks>
        ///     The returned sequence is essentially a duplicate of the original, but with the extra action
        ///     being executed while the sequence is evaluated. The action is always taken before the element is yielded,
        ///     so any changes made by the action will be visible in the returned sequence. This operator uses deferred
        ///     execution and streams its results.
        ///     The method seems simple, but it has a number of potential problems when the collection
        ///     gets mutated by the method passed to ForEach, so take care.
        /// </remarks>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The sequence of elements</param>
        /// <param name="action">The action to execute on each element</param>
        [NotNull]
        public static IEnumerable<T> ForEach<T>([NotNull] this IEnumerable<T> self, [NotNull] Action<T> action)
        {
            Check.NotNull(self)
                 .NotNull(action);

            foreach (var element in self) {
                action(element);
                yield return element;
            }
        }

        /// <summary> Prepends the specified <paramref name="items" /> to the <paramref name="self" />. </summary>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The enumerable.</param>
        /// <param name="items">The items to prepend sequence with.</param>
        /// <returns>New enumerator instance.</returns>
        [NotNull]
        public static IEnumerable<T> Prepend<T>([NotNull] this IEnumerable<T> self, [CanBeNull] T[] items)
        {
            Check.NotNull(self);

            if (items != null) foreach (var elem in items) yield return elem;

            foreach (var elem in self) yield return elem;
        }

        /// <summary>
        ///     Performs lazy splitting of the provided collection into collections of
        ///     <paramref name="sliceLength" />.
        /// </summary>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The source.</param>
        /// <param name="sliceLength">Maximum length of the slice.</param>
        /// <returns>Enumerator of slices.</returns>
        [NotNull]
        public static IEnumerable<IList<T>> Slice<T>([NotNull] this IEnumerable<T> self, int sliceLength)
        {
            Check.NotNull(self)
                 .Greater(sliceLength, 0, "Slice length is <= 0", nameof(sliceLength));

            var curr = new List<T>(sliceLength);
            foreach (var x in self) {
                curr.Add(x);
                if (curr.Count == sliceLength) {
                    yield return curr;
                    curr = new List<T>(sliceLength);
                }
            }

            if (curr.Count > 0) yield return curr;
        }

        /// <summary>
        ///     Performs lazy splitting of the provided sequence into arrays of <paramref name="sliceLength" />.
        ///     Each array will have total <em>weight</em> equal or less than <paramref name="maxSliceWeight" />.
        /// </summary>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The source collection to slice.</param>
        /// <param name="sliceLength">Length of the slice.</param>
        /// <param name="weightFunc">Function to calculate <em>weight</em> of each item in the collection</param>
        /// <param name="maxSliceWeight">The max item weight.</param>
        /// <returns>Enumerator over the results.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Weight returned by weight function is > maxSliceWeight
        /// </exception>
        [NotNull]
        public static IEnumerable<T[]> Slice<T>(
            [NotNull] this IEnumerable<T> self,
            int sliceLength,
            [NotNull] Func<T, int> weightFunc,
            int maxSliceWeight)
        {
            Check.NotNull(self)
                 .NotNull(weightFunc)
                 .Greater(sliceLength, 0, "Slice length is <= 0", nameof(sliceLength))
                 .Greater(maxSliceWeight, 0, "Max slice weight length is <= 0",
                          nameof(maxSliceWeight));

            var list = new List<T>(sliceLength);
            var accumulatedWeight = 0;

            foreach (var item in self) {
                var currentWeight = weightFunc(item);
                if (currentWeight > maxSliceWeight)
                    throw new InvalidOperationException("Weight returned by weight function is > maxSliceWeight");

                var weightOverload = currentWeight + accumulatedWeight > maxSliceWeight;
                if ((sliceLength == list.Count) || weightOverload) {
                    accumulatedWeight = 0;
                    yield return list.ToArray();
                    list.Clear();
                }

                list.Add(item);
                accumulatedWeight += currentWeight;
            }

            if (list.Count > 0) yield return list.ToArray();
        }

        /// <summary> Converts the enumerable to <see cref="HashSet{T}" /> </summary>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The enumerable to convert.</param>
        /// <returns>Created <see cref="HashSet{T}" /> instance.</returns>
        public static HashSet<T> ToSet<T>([NotNull] this IEnumerable<T> self)
        {
            Check.NotNull(self);

            return new HashSet<T>(self);
        }

        /// <summary> Converts the enumerable to <see cref="HashSet{T}" /> </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the item.</typeparam>
        /// <param name="self">The enumerable to convert.</param>
        /// <param name="selector">The selector to filter sequence.</param>
        /// <returns>Created <see cref="HashSet{T}" /> instance.</returns>
        [NotNull]
        public static HashSet<TKey> ToSet<TKey, TValue>(
            [NotNull] this IEnumerable<TValue> self,
            [NotNull] Func<TValue, TKey> selector)
        {
            Check.NotNull(self)
                 .NotNull(selector);

            return new HashSet<TKey>(self.Select(selector));
        }

        /// <summary>Returns string representations for every element of this sequence.</summary>
        /// <typeparam name="T">The type of the sequence element.</typeparam>
        /// <param name="self">The sequence of elements.</param>
        /// <param name="yieldNulls">if set to <c>true</c>, will yield strings for null elements.</param>
        /// <param name="nullRepr">
        ///     String which will be used to represent null values if <paramref name="yieldNulls" />
        ///     is set to true.
        /// </param>
        /// <returns>String representations for every element of the given sequence.</returns>
        [NotNull]
        public static IEnumerable<string> ToStrings<T>(
            [NotNull] this IEnumerable<T> self,
            bool yieldNulls = false,
            [CanBeNull] string nullRepr = "null")
        {
            Check.NotNull(self);

            var isRef = typeof(T).IsByRef || (Nullable.GetUnderlyingType(typeof(T)) != null);
            foreach (var element in self) {
                var repr = isRef && ReferenceEquals(element, null) ? nullRepr : element.ToString();
                if (yieldNulls || !string.Equals(repr, nullRepr, StringComparison.Ordinal)) yield return repr;
            }
        }

        /// <summary> Checks if two sequences are considered diffequality </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="first">The first sequence.</param>
        /// <param name="second">The second sequence.</param>
        /// <param name="checkForDuplicates">
        ///     If set to false, will allow multiple occurences of the same element,
        ///     e.g. { 1, 2, 2 } would be equal to { 1, 1, 2 }; and { 1, 2, 2, 1 } would be equal to { 2, 1 }.
        /// </param>
        /// <returns><c>true</c> if two sequences are considered equal; <c>false</c> otherwise.</returns>
        private static bool AreSequencesDifferent<T>(IEnumerable<T> first,
                                                     IEnumerable<T> second,
                                                     bool checkForDuplicates = true)
        {
            int firstNullCount;
            int secondNullCount;

            var firstElementCounts = GetCounts(first, out firstNullCount);
            var secondElementCounts = GetCounts(second, out secondNullCount);
            if ((firstNullCount != secondNullCount) || (firstElementCounts.Count != secondElementCounts.Count))
                return true;

            if (checkForDuplicates)
                foreach (var kvp in firstElementCounts) {
                    var firstElementCount = kvp.Value;
                    int secondElementCount;
                    secondElementCounts.TryGetValue(kvp.Key, out secondElementCount);

                    if (firstElementCount != secondElementCount) return true;
                }

            return false;
        }

        /// <summary>
        ///     Counts occurences of an element in the given sequence, nulls are counted separately.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="enumerable">Sequence to count.</param>
        /// <param name="nulls">Number of nulls in the sequence.</param>
        /// <returns>Dictionary [element → number of element occurences in the sequence].</returns>
        private static Dictionary<T, int> GetCounts<T>(IEnumerable<T> enumerable, out int nulls)
        {
            var dictionary = new Dictionary<T, int>();
            nulls = 0;

            foreach (var element in enumerable)
                if (ReferenceEquals(element, null)) {
                    nulls++;
                }
                else {
                    int elementCount;
                    dictionary.TryGetValue(element, out elementCount);
                    elementCount++;
                    dictionary[element] = elementCount;
                }

            return dictionary;
        }


        // Typical generic static class for easy caching of expressions.
        // ReSharper disable StaticFieldInGenericType
        private static class Source<TSource>
        {
            private static readonly ParameterExpression _Source = Expression.Parameter(typeof(TSource), "source");

            public static class CastCache<TDest>
            {
                private static readonly UnaryExpression _Cast = Expression.Convert(_Source, typeof(TDest));

                public static Func<TSource, TDest> Cast { get; } =
                    Expression.Lambda<Func<TSource, TDest>>(_Cast, _Source).Compile();
            }
        }
    }
}