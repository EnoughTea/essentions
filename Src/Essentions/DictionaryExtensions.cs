using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="IDictionary{TKey,TValue}" />. </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     Returns <paramref name="defaultValue" /> if the given <paramref name="key" />
        ///     is not present within the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary to search for value.</param>
        /// <param name="key">The key to look for.</param>
        /// <param name="defaultValue">The default value to be returned if the specified key is not present.</param>
        /// <returns>
        ///     Value matching specified <paramref name="key" /> or
        ///     <paramref name="defaultValue" /> if none is found.
        /// </returns>
        [CanBeNull]
        public static TValue GetValue<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dict,
            [NotNull] TKey key,
            [CanBeNull] TValue defaultValue = default(TValue))
        {
            Check.NotNull(dict);

            TValue value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        /// <summary>
        ///     Calls <paramref name="defaultValueProvider" /> to get a default value if the given
        ///     <paramref name="key" /> is not present within the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary to search for value.</param>
        /// <param name="key">The key to look for.</param>
        /// <param name="defaultValueProvider">
        ///     Way to get a default value to be returned if the specified key is
        ///     not present.
        /// </param>
        /// <returns>
        ///     Value matching specified <paramref name="key" /> or <paramref name="defaultValueProvider" />
        ///     if none is found.
        /// </returns>
        [CanBeNull]
        public static TValue GetValue<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dict,
            [NotNull] TKey key,
            [CanBeNull] Func<TKey, TValue> defaultValueProvider)
        {
            Check.NotNull(dict);

            TValue value;
            return dict.TryGetValue(key, out value)
                ? value
                : defaultValueProvider != null ? defaultValueProvider(key) : default(TValue);
        }

        /// <summary>
        ///     Adds the specified value to the list of the values held under the specified key.
        ///     List will be created if it does not exist.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TElement">Type of the collection element.</typeparam>
        /// <typeparam name="TCollection">Type of the inner collection.</typeparam>
        /// <param name="dict">The dictionary to add value into.</param>
        /// <param name="key">The key used for a list of values.</param>
        /// <param name="toAdd">The value to add to a list of values.</param>
        public static void AddInner<TKey, TCollection, TElement>(
            [NotNull] this IDictionary<TKey, TCollection> dict,
            [NotNull] TKey key,
            [CanBeNull] TElement toAdd)
            where TCollection : ICollection<TElement>
        {
            Check.NotNull(dict);

            var existing = GetOrCreateInnerCollection(dict, key);
            existing.Add(toAdd);
        }

        /// <summary>
        ///     Adds the specified value to the set of the values held under the specified key.
        ///     Set will be created if it does not exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary to add value into.</param>
        /// <param name="key">The key used for a list of values.</param>
        /// <param name="toAdd">The value to add to a set of values.</param>
        public static bool AddInner<TKey, TValue>(
            [NotNull] this IDictionary<TKey, HashSet<TValue>> dict,
            [NotNull] TKey key,
            [CanBeNull] TValue toAdd)
        {
            Check.NotNull(dict);

            var existing = GetOrCreateInnerCollection(dict, key);
            return existing.Add(toAdd);
        }

        /// <summary>
        ///     Adds the specified value to the set of the values held under the specified key.
        ///     Set will be created if it does not exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary to add value into.</param>
        /// <param name="key">The key used for a list of values.</param>
        /// <param name="toAdd">The value to add to a set of values.</param>
        public static bool AddInner<TKey, TValue>(
            [NotNull] this IDictionary<TKey, ISet<TValue>> dict,
            [NotNull] TKey key,
            [CanBeNull] TValue toAdd)
        {
            Check.NotNull(dict);

            var existing = GetOrCreateInnerCollection(dict, key);
            return existing.Add(toAdd);
        }

        /// <summary> Adds the specified values to the set of the values held under the specified key. </summary>
        /// <param name="dict">The dictionary to add values into.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="toAdd">Values to add.</param>
        public static void AddInnerRange<TKey, TCollection, TElement>(
                    [NotNull] this IDictionary<TKey, TCollection> dict,
                    [NotNull] TKey key,
                    [CanBeNull] IEnumerable<TElement> toAdd)
                    where TCollection : ICollection<TElement>
        {
            Check.NotNull(dict);

            if (toAdd == null) {
                return;
            }

            var existing = GetOrCreateInnerCollection(dict, key);
            existing.AddRange(toAdd);
        }

        /// <summary> Adds the specified values to the set of the values held under the specified key. </summary>
        /// <param name="dict">The dictionary with inner hashsets to add values into.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="values">Values to add.</param>
        public static void AddInnerRange<TKey, TValue>(
            [NotNull] this IDictionary<TKey, HashSet<TValue>> dict,
            [NotNull] TKey key,
            [CanBeNull] IEnumerable<TValue> values)
        {
            Check.NotNull(dict);

            if (values == null) return;

            var existing = GetOrCreateInnerCollection(dict, key);
            existing.UnionWith(values);
        }

        /// <summary> Adds the specified values to the set of the values held under the specified key. </summary>
        /// <param name="dict">The dictionary with inner hashsets to add values into.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="values">Values to add.</param>
        public static void AddInnerRange<TKey, TValue>(
            [NotNull] this IDictionary<TKey, ISet<TValue>> dict,
            [NotNull] TKey key,
            [CanBeNull] IEnumerable<TValue> values)
        {
            Check.NotNull(dict);

            if (values == null) return;

            var existing = GetOrCreateInnerCollection(dict, key);
            existing.UnionWith(values);
        }

        /// <summary>
        ///     Adds the specified value to the list of the values held under the specified key.
        ///     List will be created if it does not exist.
        /// </summary>
        /// <param name="dict">The dictionary to add value into.</param>
        /// <param name="key">The key used for a list of values.</param>
        /// <param name="toAdd">The value to add to a list of values.</param>
        public static bool AddUniqueInner<TKey, TCollection, TElement>(
            [NotNull] this IDictionary<TKey, TCollection> dict,
            [NotNull] TKey key,
            [CanBeNull] TElement toAdd)
            where TCollection : ICollection<TElement>
        {
            Check.NotNull(dict);

            var existing = GetOrCreateInnerCollection(dict, key);
            return existing.AddUnique(toAdd);
        }

        /// <summary>
        ///     Determines whether the specified value is contained in the collection of the values associated
        ///     with the given key.
        /// </summary>
        /// <param name="dict">The dictionary with inner hashsets to check.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="value">Value to check for.</param>
        /// <returns>
        ///     true if the value is contained in the collection of values a associated with the given key;
        ///     false otherwise.
        /// </returns>
        public static bool ContainsInner<TKey, TCollection, TElement>(
            [NotNull] this IDictionary<TKey, TCollection> dict,
            [NotNull] TKey key,
            [CanBeNull] TElement value)
            where TCollection : ICollection<TElement>
        {
            Check.NotNull(dict);

            var contains = false;
            TCollection values;
            if (dict.TryGetValue(key, out values)) {
                contains = values.Contains(value);
            }

            return contains;
        }

        /// <summary>
        ///     Removes the specified value from the collection of values associated with the given key.
        ///     When there won't be any values left for the key, key will be removed as well.
        /// </summary>
        /// <param name="dict">The dictionary with inner hashsets to remove from.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="value">Value to remove.</param>
        /// <returns>true if the value was removed from this dictionary; false key or value was not found.</returns>
        public static bool RemoveInner<TKey, TCollection, TElement>(
            [NotNull] this IDictionary<TKey, TCollection> dict,
            [NotNull] TKey key,
            [CanBeNull] TElement value)
            where TCollection : ICollection<TElement>
        {
            Check.NotNull(dict);

            var removed = false;
            TCollection storage;
            if (dict.TryGetValue(key, out storage)) {
                removed = storage.Remove(value);
                if (storage.Count == 0) dict.Remove(key);
            }

            return removed;
        }

        [NotNull]
        private static TCollection GetOrCreateInnerCollection<TKey, TCollection>(
            [NotNull] IDictionary<TKey, TCollection> dict,
            [NotNull] TKey key,
            [CanBeNull] Func<TCollection> creator = null)
        {
            Check.NotNull(dict);

            TCollection existing;
            if (!dict.TryGetValue(key, out existing) || (existing == null)) {
                var created = creator != null ? creator() : Activator.CreateInstance<TCollection>();
                if (created == null)
                    throw new InvalidOperationException("Creator function for inner collection returned null.");

                existing = created;
                dict[key] = existing;
            }

            return existing;
        }
    }
}